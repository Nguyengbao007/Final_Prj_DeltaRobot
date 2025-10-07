"""Tkinter GUI to exercise the CodeDelta_Nhom_TamBui.Kinematic module."""

from __future__ import annotations

import os
import sys
import tkinter as tk
from typing import Sequence

try:
    from .Kinematic import forward_kinematic, inverse_kinematic
except ImportError:  # Support running as a stand-alone script when the package is not installed
    CURRENT_DIR = os.path.dirname(os.path.abspath(__file__))
    if CURRENT_DIR not in sys.path:
        sys.path.append(CURRENT_DIR)
    from Kinematic import forward_kinematic, inverse_kinematic


def _format(values: Sequence[float]) -> Sequence[str]:
    return [f"{value:.6f}" for value in values]


def launch_gui() -> None:
    """Create and start the Tkinter window."""
    root = tk.Tk()
    root.title("Delta Robot Kinematics Tester")
    root.resizable(False, False)

    theta_labels = ("Theta 1 (deg)", "Theta 2 (deg)", "Theta 3 (deg)")
    xyz_labels = ("X (mm)", "Y (mm)", "Z (mm)")

    theta_vars = [tk.StringVar(value="0.0") for _ in range(3)]
    xyz_vars = [tk.StringVar(value="0.0") for _ in range(3)]

    status_var = tk.StringVar(value="Ready")

    def parse_entries(variables: Sequence[tk.StringVar]) -> Sequence[float]:
        parsed = []
        for var in variables:
            raw = var.get().strip()
            if not raw:
                raise ValueError("Inputs must not be empty.")
            parsed.append(float(raw))
        return parsed

    def put_values(target: Sequence[tk.StringVar], values: Sequence[float]) -> None:
        for var, formatted in zip(target, _format(values)):
            var.set(formatted)

    def on_inverse() -> None:
        try:
            x_val, y_val, z_val = parse_entries(xyz_vars)
            thetas = inverse_kinematic(x_val, y_val, z_val)
        except Exception as exc:  # Show any numeric issues to the user
            status_var.set(f"Inverse error: {exc}")
        else:
            put_values(theta_vars, thetas)
            status_var.set("Inverse computation complete.")

    def on_forward() -> None:
        try:
            t1, t2, t3 = parse_entries(theta_vars)
            success, x_val, y_val, z_val = forward_kinematic(t1, t2, t3)
            if not success:
                raise ValueError("Configuration is not reachable.")
        except Exception as exc:
            status_var.set(f"Forward error: {exc}")
        else:
            put_values(xyz_vars, (x_val, y_val, z_val))
            status_var.set("Forward computation complete.")

    container = tk.Frame(root, padx=16, pady=16)
    container.grid(row=0, column=0, sticky="nsew")

    angles_frame = tk.LabelFrame(container, text="Joint Angles", padx=8, pady=8)
    angles_frame.grid(row=0, column=0, sticky="n")

    buttons_frame = tk.Frame(container, padx=12)
    buttons_frame.grid(row=0, column=1, sticky="ns")

    pose_frame = tk.LabelFrame(container, text="End Effector", padx=8, pady=8)
    pose_frame.grid(row=0, column=2, sticky="n")

    for row, (label, var) in enumerate(zip(theta_labels, theta_vars)):
        tk.Label(angles_frame, text=label).grid(row=row, column=0, sticky="w", pady=4, padx=(0, 6))
        tk.Entry(angles_frame, textvariable=var, width=14, justify="right").grid(row=row, column=1, pady=4)

    for row, (label, var) in enumerate(zip(xyz_labels, xyz_vars)):
        tk.Label(pose_frame, text=label).grid(row=row, column=0, sticky="w", pady=4, padx=(0, 6))
        tk.Entry(pose_frame, textvariable=var, width=14, justify="right").grid(row=row, column=1, pady=4)

    tk.Button(buttons_frame, text="Inverse", width=14, command=on_forward).grid(row=0, column=0, pady=(0, 8))
    tk.Button(buttons_frame, text="Forward", width=14, command=on_inverse).grid(row=1, column=0)

    tk.Label(container, textvariable=status_var, anchor="w").grid(row=1, column=0, columnspan=3, sticky="ew", pady=(12, 0))

    root.mainloop()


if __name__ == "__main__":
    launch_gui()
