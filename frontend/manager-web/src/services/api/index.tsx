import axios, { AxiosError, AxiosResponse } from "axios";
import UnAuthoraizedToasts from "components/toasts/UnAuthoraizedToast";

import { ToastContentProps, toast, ToastPromiseParams, ToastOptions } from "react-toastify";
import { AUTH_PATH, ME_PATH } from "./auth";
import EventDomDispatcher from "services/events/EventDomDispatcher";
import { LOGOUT_EVENT } from "services/events/consts";
import { ROUTES } from "router/enums";

export const getToken = () => window.localStorage.getItem("token");
export const createBaseURL = (url = null) => {
  const isLocalhost = window.location.hostname === "localhost";

  const DOMAIN_LENGTH = 2;
  const API_PREFIX = "api";
  const SEPARATOR = ".";
  const origin = isLocalhost ? process.env.REACT_APP_API_URL : window.location.origin;

  const hostParts = origin.split(SEPARATOR).filter(part => part !== API_PREFIX);
  const domain = hostParts.slice(hostParts.length - DOMAIN_LENGTH);
  const projectType = hostParts.slice(0, hostParts.length - DOMAIN_LENGTH);
  const [envType] = projectType;
  const ApiType = url ? [envType, url] : projectType;
  const baseURL = [...ApiType, API_PREFIX, ...domain].join(SEPARATOR);
  return baseURL;
};

const getBaseInstance = () => axios.create();

export const makeInstance = () => {
  const baseInstance = getBaseInstance();
  const baseUrl = createBaseURL();
  baseInstance.interceptors.request.use(
    config => {
      config.baseURL = config.baseURL || baseUrl;
      config.headers.set("Content-type", "application/json");
      config.headers.set("Authorization", `Bearer ${getToken()}`);
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
      if (error.code === "ERR_CANCELED") return;
      if (error.config.url === AUTH_PATH) return;
      // this is hidden check auth request
      // huck to hide error on auth page (401)
      if (error.config.url === ME_PATH && window.location.pathname === `/${ROUTES.AUTH}`) {
        throw error;
      }
      if (!error.response) {
        return handlePromiseError(Promise.reject(error));
      }

      if (error.response.status === 401) {
        const { dispatchEvent } = EventDomDispatcher();
        dispatchEvent(new CustomEvent(LOGOUT_EVENT));
      }

      return handlePromiseError(Promise.reject(error));
    }
  );

  return baseInstance;
};
const baseAxiosInstanse = makeInstance();

export default baseAxiosInstanse;

export const handlePromiseError = <Handler extends (params) => string>(
  response: Promise<AxiosResponse>,
  errorSchemaHandler?: Handler
) => {
  const toastOptions: ToastOptions = {};
  const handlers: ToastPromiseParams = {
    error: {
      render: (
        props: ToastContentProps<AxiosError<{ errors: Record<string, string[]>; title: string }>>
      ) => {
        const status = props.data.response.status;
        const data = props.data.response.data;
        if (status >= 500) return "The server is unavailable at the moment";
        if (status === 401) {
          return <UnAuthoraizedToasts {...props} />;
        }
        return errorSchemaHandler
          ? errorSchemaHandler(props.data.response.data)
          : data?.title || "Something went wrong...";
      },
    },
  };
  response.catch(error => {
    if (error.response.status === 401) {
      toastOptions.toastId = "unauthorizedToastID";
    }
    toast.promise(response, handlers, toastOptions);
  });
  return response;
};
