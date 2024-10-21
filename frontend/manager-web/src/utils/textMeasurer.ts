let canvas = null;

export function getTextWidth(text, font) {
  canvas = canvas || document.createElement("canvas");
  const context = canvas.getContext("2d");
  context.font = font;

  const metrics = context.measureText(text);
  return metrics.width;
}

export function getElFont(el = document.body) {
  const fontWeight = getCssStyle(el, "font-weight");
  const fontSize = getCssStyle(el, "font-size");
  const fontFamily = getCssStyle(el, "font-family");

  return `${fontWeight} ${fontSize} ${fontFamily}`;
}

function getCssStyle(element, prop) {
  return window.getComputedStyle(element, null).getPropertyValue(prop);
}
