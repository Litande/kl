import axios, { AxiosError, AxiosResponse } from "axios";
import { ToastContentProps, toast, ToastPromiseParams } from "react-toastify";
import UnAuthoraizedToasts from "components/toasts/UnAuthoraizedToast";
import EventDomDispatcher from "services/events/EventDomDispatcher";
import { LOGOUT_EVENT } from "services/events/consts";
import { NO_INTERNET_CONNECTION_ERROR } from "components/connection/utils";

export const getToken = () => {
  return window.localStorage.getItem("token");
};

export const createBaseURL = () => {
  const isLocalhost = window.location.hostname === "localhost";
  const DOMAIN_LENGTH = 2;
  const API_PREFIX = "api";
  const SEPARATOR = ".";
  const protocol = window.location.protocol + "//";
  const hostParts = window.location.host.split(SEPARATOR);
  const domain = hostParts.slice(hostParts.length - DOMAIN_LENGTH);
  const projectType = hostParts.slice(0, hostParts.length - DOMAIN_LENGTH);
  const baseURL = protocol + [...projectType, API_PREFIX, ...domain].join(SEPARATOR);
  return isLocalhost ? process.env.REACT_APP_API_URL : baseURL;
};

const getBaseInstance = () => axios.create();

const makeInstance = () => {
  const baseInstance = getBaseInstance();
  baseInstance.interceptors.request.use(
    config => {
      config.baseURL = createBaseURL();
      config.headers = {
        "Content-type": "application/json",
        Authorization: `Bearer ${getToken()}`,
      };
      return config;
    },
    error => {
      return Promise.reject(error);
    }
  );

  baseInstance.interceptors.response.use(
    response => {
      return response;
    },
    async function (error) {
      if (!error.response) {
        return handlePromiseError(Promise.reject(error));
      }

      if (error.response.status === 401) {
        const { dispatchEvent } = EventDomDispatcher();
        window.localStorage.removeItem("token");
        window.localStorage.removeItem("auth");
        dispatchEvent(new CustomEvent(LOGOUT_EVENT));
      }

      return handlePromiseError(Promise.reject(error));
    }
  );

  return baseInstance;
};

export default makeInstance();

export const handlePromiseError = <Handler extends (params) => string>(
  response: Promise<AxiosResponse>,
  errorSchemaHandler?: Handler
) => {
  const handlers: ToastPromiseParams = {
    error: {
      render: (
        props: ToastContentProps<AxiosError<{ error: Record<string, string[]>; title: string }>>
      ) => {
        if (props.data?.code === "ERR_NETWORK") {
          return NO_INTERNET_CONNECTION_ERROR.message;
        }
        const status = props.data.response.status;
        const data = props.data.response.data;
        if (status >= 500) return "The server is unavailable at the moment";
        if (status === 401) {
          return <UnAuthoraizedToasts {...props} />;
        }
        return (data?.error && String(data.error)) || "Something went wrong...";
      },
    },
  };
  toast.promise(response, handlers);
  return response;
};
