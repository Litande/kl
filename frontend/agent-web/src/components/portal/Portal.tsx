import React, { useEffect } from "react";
import { createPortal } from "react-dom";

export enum PORTAL_ID {
  MODAL = "modal-root",
  TOOLTIP = "tooltip-root",
}

interface IProps {
  children: React.ReactNode;
  id: PORTAL_ID;
}

const Portal = ({ children, id }: IProps) => {
  const mount = document.getElementById(id);
  const el = document.createElement("div");

  useEffect(() => {
    mount.appendChild(el);

    return () => {
      mount.removeChild(el);
    };
  }, []);

  return createPortal(children, mount);
};

export default Portal;