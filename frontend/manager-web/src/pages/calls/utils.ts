export const getBlobUrl = audioCall => {
  const blob = new Blob([audioCall], { type: "audio/ogg" });
  return URL.createObjectURL(blob);
};
