export const convertSecondsToTimeString = value => {
  if (!value) return "00:00:00";

  const hours = Math.floor(value / 3600);
  const minutes = Math.floor((value % 3600) / 60);
  const seconds = value % 60;

  return (
    hours + ":" + minutes.toString().padStart(2, "0") + ":" + seconds.toString().padStart(2, "0")
  );
};
