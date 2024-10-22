import axios, { AxiosError, AxiosResponse } from "axios";
import UnAuthoraizedToasts from "components/toasts/UnAuthoraizedToast";

import { ToastContentProps, toast, ToastPromiseParams, ToastOptions } from "react-toastify";
import { AUTH_PATH, ME_PATH } from "./auth";
import EventDomDispatcher from "services/events/EventDomDispatcher";
import { LOGOUT_EVENT } from "services/events/consts";
import { ROUTES } from "router/enums";

export const getToken = () => window.localStorage.getItem("token");

const getBaseInstance = () => axios.create();

export const makeInstance = (baseUrl) => {
  const baseInstance = getBaseInstance();
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

const axiosInstance = makeInstance(process.env.REACT_APP_API_URL);
const statsAxiosInstance = makeInstance(process.env.REACT_APP_API_URL_STATS)

export default axiosInstance;

export const statsAxios = statsAxiosInstance;

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
