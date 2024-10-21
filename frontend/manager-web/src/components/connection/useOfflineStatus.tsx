import { useState, useEffect } from "react";

const hasBrowserCompatibility = browser => {
  return !/Windows.*Chrome|Windows.*Firefox|Linux.*Chrome/.test(browser);
};

const PING_URL = "/check.txt";
const TIMEOUT = 5000;
const INTERVAL = 5000;

const ping = (url, timeout) => {
  return new Promise(resolve => {
    const success = () => resolve(true);
    const error = () => resolve(false);

    const xhr = new XMLHttpRequest();

    xhr.onerror = error;
    xhr.ontimeout = error;
    xhr.onreadystatechange = () => {
      if (xhr.readyState === xhr.HEADERS_RECEIVED) {
        if (xhr.status) {
          success();
        } else {
          error();
        }
      }
    };

    xhr.open("GET", url);
    xhr.timeout = timeout;
    xhr.send();
  });
};

const useOfflineStatus = () => {
  const [isOffline, setIsOffline] = useState(
    typeof navigator !== "undefined" && typeof navigator.onLine === "boolean"
      ? !navigator.onLine
      : false
  );

  useEffect(() => {
    let intervalId = null;

    window.addEventListener("offline", () => {
      setIsOffline(true);
    });

    window.addEventListener("online", () => {
      setIsOffline(false);
    });

    if (typeof navigator !== "undefined" && !hasBrowserCompatibility(navigator.userAgent)) {
      intervalId = setInterval(async () => {
        const isOnline = await ping(PING_URL, TIMEOUT);

        setIsOffline(!isOnline);
      }, INTERVAL);
    }

    return () => {
      window.removeEventListener("offline", () => {
        setIsOffline(true);
      });

      window.removeEventListener("online", () => {
        setIsOffline(false);
      });

      intervalId && clearInterval(intervalId);
    };
  }, []);

  return isOffline;
};

export default useOfflineStatus;
