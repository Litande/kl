const downloadFile = (linkSource, fileName = "document.pdf") => {
  const downloadLink = document.createElement("a");

  downloadLink.href = linkSource;
  downloadLink.download = fileName;
  downloadLink.click();
};

export default downloadFile;
