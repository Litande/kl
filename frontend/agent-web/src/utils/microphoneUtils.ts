import CallError from "data/errors/CallError";

export const getMicrophonePermission = async () => {
  return navigator.permissions.query({ name: "microphone" }).then(({ state }) => state);
};

export const askMicrophonePermission = async () => {
  await navigator.mediaDevices.getUserMedia({
    video: false,
    audio: true,
  });
};

export const handleMicrophonePermission = async (onSuccess, onError) => {
  try {
    await askMicrophonePermission();
  } catch (e) {
    onError && onError();
    return;
  }
  const micPermission = await getMicrophonePermission();

  if (micPermission === "denied" || micPermission === "prompt") {
    onError && onError();
  } else {
    onSuccess && onSuccess();
  }
};

export const MICROPHONE_ERROR = new CallError(
  "1002",
  "Microphone unavailable, please check you settings in the browser."
);
