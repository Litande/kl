import {
  ColorsEnum,
  CONNECTION_STATUS,
  IFillStatus,
  OpacityEnum,
} from "components/connection/types";

export const FillMapByStatus: Partial<Record<CONNECTION_STATUS, IFillStatus>> = {
  [CONNECTION_STATUS.STABLE_CONNECTION]: {
    line1: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    line2: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    line3: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    line4: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    dotLine: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    description: "Stable Connection",
  },
  [CONNECTION_STATUS.GOOD_CONNECTION]: {
    line1: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line2: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    line3: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    line4: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    dotLine: {
      fill: ColorsEnum.SUCCESS,
      opacity: OpacityEnum.SUCCESS,
    },
    description: "Stable Connection",
  },
  [CONNECTION_STATUS.POOR_CONNECTION]: {
    line1: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line2: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line3: {
      fill: ColorsEnum.UNSTABLE,
      opacity: OpacityEnum.SUCCESS,
    },
    line4: {
      fill: ColorsEnum.UNSTABLE,
      opacity: OpacityEnum.SUCCESS,
    },
    dotLine: {
      fill: ColorsEnum.UNSTABLE,
      opacity: OpacityEnum.SUCCESS,
    },
    description: "Unstable Connection",
  },
  [CONNECTION_STATUS.UNSTABLE_CONNECTION]: {
    line1: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line2: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line3: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line4: {
      fill: ColorsEnum.UNSTABLE,
      opacity: OpacityEnum.SUCCESS,
    },
    dotLine: {
      fill: ColorsEnum.UNSTABLE,
      opacity: OpacityEnum.SUCCESS,
    },
    description: "Unstable Connection",
  },
  [CONNECTION_STATUS.NO_CONNECTION]: {
    line1: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line2: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line3: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    line4: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    dotLine: {
      fill: ColorsEnum.FAIL,
      opacity: OpacityEnum.FAIL,
    },
    noConnect: {
      fill: ColorsEnum.RED,
      opacity: OpacityEnum.SUCCESS,
    },
    description: "Disconnected",
  },
};
