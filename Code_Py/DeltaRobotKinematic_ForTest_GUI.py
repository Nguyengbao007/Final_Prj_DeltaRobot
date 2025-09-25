"""Tkinter GUI for delta robot forward and inverse kinematics."""

import sys
import tkinter as tk
from typing import Optional, Sequence

try:
    from .DeltaRobotKinematics import DeltaDimensions, DeltaRobot
except ImportError:  # When executed as a script
    from DeltaRobotKinematics import DeltaDimensions, DeltaRobot

# Map old-style params to new geometry fields
DEFAULT_DIMENSIONS = DeltaDimensions(
    base_radius_f=75.0,             # corresponds to old f
    end_effector_radius_e=24.0,     # corresponds to old e
    bicep_length_rf=100.0,          # rf
    forearm_length_re=300.0         # re
)


def _format_values(values: Sequence[float]) -> Sequence[str]:
    return [f"{value:.6f}" for value in values]


def launch_gui(robot: Optional[DeltaRobot] = None) -> None:
    """Launch the GUI window."""
    if robot is None:
        dims = DeltaDimensions(**DEFAULT_DIMENSIONS.__dict__)
        robot = DeltaRobot(dims, min_angle_deg=-75.0, max_angle_deg=75.0)

    root = tk.Tk()
    root.title("Delta Robot Kinematics")
    root.resizable(False, False)

    theta_labels = ("Theta 1 (deg)", "Theta 2 (deg)", "Theta 3 (deg)")
    xyz_labels = ("X (mm)", "Y (mm)", "Z (mm)")

    theta_vars = [tk.StringVar(value="0.0") for _ in range(3)]
    xyz_vars = [tk.StringVar(value="0.0") for _ in range(3)]

    root.columnconfigure(0, weight=1)
    root.rowconfigure(0, weight=1)

    main_frame = tk.Frame(root, padx=16, pady=16)
    main_frame.grid(row=0, column=0, sticky="nsew")

    inputs_frame = tk.Frame(main_frame)
    inputs_frame.grid(row=0, column=0, sticky="nsew")

    angle_frame = tk.LabelFrame(inputs_frame, text="Joint Angles", padx=8, pady=8)
    angle_frame.grid(row=0, column=0, sticky="n")

    buttons_frame = tk.Frame(inputs_frame, pady=4)
    buttons_frame.grid(row=0, column=1, sticky="ns", padx=12)

    position_frame = tk.LabelFrame(inputs_frame, text="End Effector", padx=8, pady=8)
    position_frame.grid(row=0, column=2, sticky="n")

    for idx, (label_text, var) in enumerate(zip(theta_labels, theta_vars)):
        tk.Label(angle_frame, text=label_text).grid(row=idx, column=0, sticky="w", pady=4, padx=(0, 6))
        tk.Entry(angle_frame, textvariable=var, width=14, justify="right").grid(row=idx, column=1, pady=4)

    for idx, (label_text, var) in enumerate(zip(xyz_labels, xyz_vars)):
        tk.Label(position_frame, text=label_text).grid(row=idx, column=0, sticky="w", pady=4, padx=(0, 6))
        tk.Entry(position_frame, textvariable=var, width=14, justify="right").grid(row=idx, column=1, pady=4)

    status_var = tk.StringVar(value="Ready")

    def parse(var_list):
        values = []
        for variable in var_list:
            text_value = variable.get().strip()
            if not text_value:
                raise ValueError("Input values must not be empty.")
            values.append(float(text_value))
        return values

    def apply_thetas(values):
        for variable, formatted in zip(theta_vars, _format_values(values)):
            variable.set(formatted)

    def apply_xyz(values):
        for variable, formatted in zip(xyz_vars, _format_values(values)):
            variable.set(formatted)

    def handle_inverse():
        try:
            x_val, y_val, z_val = parse(xyz_vars)
            thetas = robot.inverse_kinematics(x_val, y_val, z_val)
        except Exception as exc:
            status_var.set(f"Inverse error: {exc}")
        else:
            apply_thetas(thetas)
            status_var.set("Inverse kinematics complete.")

    def handle_forward():
        try:
            theta_vals = parse(theta_vars)
            position = robot.forward_kinematics(*theta_vals)
        except Exception as exc:
            status_var.set(f"Forward error: {exc}")
        else:
            apply_xyz(position)
            status_var.set("Forward kinematics complete.")

    tk.Button(buttons_frame, text="Inverse", width=14, command=handle_inverse).grid(row=0, column=0, pady=(0, 8))
    tk.Button(buttons_frame, text="Reverse", width=14, command=handle_forward).grid(row=1, column=0)

    tk.Label(main_frame, textvariable=status_var, anchor="w").grid(row=1, column=0, sticky="ew", pady=(12, 0))

    inputs_frame.columnconfigure(0, weight=1)
    inputs_frame.columnconfigure(1, weight=0)
    inputs_frame.columnconfigure(2, weight=1)
    main_frame.columnconfigure(0, weight=1)

    root.mainloop()

if __name__ == "__main__":
    try:
        launch_gui()
    except Exception as exc:  # pragma: no cover - GUI errors to console
        print(f"Failed to launch GUI: {exc}", file=sys.stderr)
        raise








