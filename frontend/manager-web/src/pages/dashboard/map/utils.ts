export type ColorValueHex = string;
type GetColor = (n: number) => ColorValueHex;
export const createColorFunction = (
  arr: number[],
  minColor: ColorValueHex,
  maxColor: ColorValueHex
): GetColor => {
  // if array is empty, return min color always
  if (!arr || arr.length === 0) {
    return _ => minColor;
  }

  // find the highest and lowest number in the array
  const max = Math.max(...arr);
  const min = Math.min(...arr);

  // calculate the range of values
  const range = max - min;

  // return a function that maps a number to a hex color
  return (num): ColorValueHex => {
    // calculate the value's position within the range
    const position = (num - min) / range;

    // interpolate between the min and max colors based on the value's position
    const r = Math.round(
      (1 - position) * parseInt(minColor.substring(1, 3), 16) +
        position * parseInt(maxColor.substring(1, 3), 16)
    );
    const g = Math.round(
      (1 - position) * parseInt(minColor.substring(3, 5), 16) +
        position * parseInt(maxColor.substring(3, 5), 16)
    );
    const b = Math.round(
      (1 - position) * parseInt(minColor.substring(5), 16) +
        position * parseInt(maxColor.substring(5), 16)
    );

    // format the color as a hex string
    return "#" + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
  };
};
