import { getTextWidth, getElFont } from "utils/textMeasurer";

type getAmountToRenderType = (
  isFontLoaded: boolean,
  tags: { id: number; name: string; value: number }[],
  containerRef: { current: HTMLDivElement }
) => number;

export const getAmountToRender: getAmountToRenderType = (isFontLoaded, tags, containerRef) => {
  if (!isFontLoaded || !containerRef.current) return tags.length;

  const containerWidth = containerRef.current.offsetWidth;
  const [paddings, margins, moreTagsWidth] = [32, 7, 20];

  const font = getElFont(containerRef.current);

  const tagsWidth = tags.map(tag => getTextWidth(tag.name, font) + paddings + margins);

  return getAmount(tagsWidth, containerWidth - moreTagsWidth);
};

function getAmount(widths, totalWidth) {
  let amount = 0;
  let width = 0;
  let isMaxAmount = false;

  widths.forEach(item => {
    if (isMaxAmount) return;

    if (totalWidth - width - item > 0) {
      width += item;
      amount++;
    } else {
      isMaxAmount = true;
    }
  });

  return amount;
}
