import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { ToastContentProps } from "react-toastify";

import { ROUTES } from "router/enums";

const UnAuthoraizedToasts = (props: ToastContentProps) => {
  const navigate = useNavigate();

  useEffect(() => {
    setTimeout(() => {
      navigate(ROUTES.AUTH);
    }, (props.toastProps.autoClose as number) || 1000);
  }, []);

  return <div>Your session has been expired. You will be moved to login page</div>;
};

export default UnAuthoraizedToasts;
