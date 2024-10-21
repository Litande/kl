import InterLight from "fonts/Inter-Light.ttf";
import InterRegular from "fonts/Inter-Regular.ttf";
import InterMedium from "fonts/Inter-Medium.ttf";
import { css } from "styled-components";

const fontsStyles = css`
  @font-face {
    font-family: "Inter light";
    src: url(${InterLight}) format("truetype");
  }
  @font-face {
    font-family: "Inter italic";
    font-style: italic;
    src: url(${InterLight}) format("truetype");
  }
  @font-face {
    font-family: "Inter regular";
    src: url(${InterRegular}) format("truetype");
  }
  @font-face {
    font-family: "Inter medium";
    src: url(${InterMedium}) format("truetype");
  }

  h1 {
    font-family: "Inter medium";
    font-size: 32px;
    line-height: 39px;
  }
  h2 {
    font-family: "Inter regular";
    font-size: 28px;
    line-height: 34px;
  }
  h3 {
    font-family: "Inter regular";
    font-size: 24px;
    line-height: 29px;
  }
  h4 {
    font-family: "Inter medium";
    font-size: 24px;
    line-height: 29px;
  }
  h5 {
    font-family: "Inter light";
    font-size: 24px;
    line-height: 29px;
  }
`;

export const typography = {
  subtitle1: css`
    font-family: "Inter regular";
    font-size: 18px;
    line-height: 22px;
  `,
  subtitle2: css`
    font-family: "Inter medium";
    font-size: 18px;
    line-height: 22px;
  `,
  subtitle3: css`
    font-family: "Inter regular";
    font-size: 16px;
    line-height: 19px;
  `,
  subtitle4: css`
    font-family: "Inter medium";
    font-size: 16px;
    line-height: 19px;
  `,
  body1: css`
    font-size: 14px;
    line-height: 17px;
    font-family: "Inter regular";
  `,
  body2: css`
    font-size: 14px;
    line-height: 17px;
    font-family: "Inter medium";
  `,
  body3: css`
    font-size: 14px;
    line-height: 17px;
    font-family: "Inter light";
  `,
  body4: css`
    font-size: 14px;
    line-height: 17px;
    font-family: "Inter italic";
  `,
  smallText1: css`
    font-family: "Inter regular";
    font-size: 12px;
    line-height: 15px;
  `,
  smallText2: css`
    font-family: "Inter medium";
    font-size: 12px;
    line-height: 15px;
  `,
  smallText3: css`
    font-family: "Inter light";
    font-size: 12px;
    line-height: 15px;
  `,
  buttonsText: css`
    font-family: "Inter regular";
    font-size: 16px;
    line-height: 19px;
  `,
  buttonsText5: css`
    font-family: "Inter regular";
    font-size: 10px;
    line-height: 12px;
  `,
};

export default fontsStyles;
