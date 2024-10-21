import { useRef } from "react";

type IMakeRequestWithAttempts = {
  request: () => void;
  onBegin?: () => void;
  onError?: () => void;
};

const MAX_ATTEMPTS = 1;
const ATTEMPT_WAIT_MILLISECONDS = 5000;

const useMakeRequestWithAttempts = (
  maxAttempts = MAX_ATTEMPTS,
  wait = ATTEMPT_WAIT_MILLISECONDS
) => {
  const attemptRef = useRef(maxAttempts);

  const makeRequestWithAttempts = async ({
    request,
    onBegin,
    onError,
  }: IMakeRequestWithAttempts) => {
    attemptRef.current = MAX_ATTEMPTS;

    onBegin && onBegin();

    const makeRequest = async () => {
      try {
        await request();
      } catch (e) {
        attemptRef.current--;

        if (attemptRef.current === 0) {
          setTimeout(async () => {
            await makeRequest();
          }, wait);
        } else {
          onError && onError();
        }
      }
    };

    await makeRequest();
  };

  return {
    makeRequestWithAttempts,
  };
};

export default useMakeRequestWithAttempts;
