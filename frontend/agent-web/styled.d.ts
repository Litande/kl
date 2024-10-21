import { typography } from "globalStyles/theme/fonts";
import { colors } from "globalStyles/theme/palette";

declare module "styled-components" {
  // eslint-disable-next-line @typescript-eslint/naming-convention
  export interface DefaultTheme {
    typography: typeof typography;
    colors: typeof colors;
  }
}
