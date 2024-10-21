import { hexToRgbA } from "utils/hexToRgbA";
import { AgentStatusStr } from "../../types";

const lightGreen = "#64C458";
const lightRed = "#FF7A00";
export const colors = {
  bg: {
    primary: "#1E1954",
    secondary: "#FBFBFC",
    ternary: "#FFFFFF",
    active: "#f3f8ff",
    light: "#F7F7F7",
    secondary_light: "#FBFBFC",
    pageHeader: "#EAE8F6",
    divider: hexToRgbA("#000", 0.08),
    tag: "#EFEFEF",
  },
  fg: {
    primary: "#1E1954",
    primary_disabled: "#1E1954",
    secondary: "#000000",
    secondary_active: "#ffffff",
    secondary_disabled: hexToRgbA("#000", 0.5),
    secondary_light: hexToRgbA("#000", 0.6),
    secondary_light_disabled: hexToRgbA("#000", 0.3),
    link: "#5142AE",
    link_hovered: "#6E5ECC",
    link_pressed: "#8774FB",
    link_disabled: hexToRgbA("#6E5ECC", 0.5),
  },
  error: "#B3261E",
  btn: {
    primary: hexToRgbA("#fff", 0.9),
    primary_hovered: "#ffffff",
    primary_pressed: "#ffffff",
    primary_disabled: hexToRgbA("#fff", 0.5),
    secondary: "#5294C3",
    secondary_off: "#D9D9D9",
    secondary_hovered: "#467EA6",
    secondary_pressed: "#467EA6",
    secondary_non_active: "#F3F8FF",
    secondary_disabled: hexToRgbA("#5294C3", 0.5),
    tertiary: "#8774FB",
    tertiary_hovered: "#6E5ECC",
    tertiary_pressed: "#6E5ECC",
    tertiary_disabled: hexToRgbA("#8774FB", 0.5),
    error_secondary: "#C80048",
    error_secondary_hovered: "#E00051",
    error_secondary_pressed: "#AD003E",
    error_secondary_disabled: hexToRgbA("#C80048", 0.3),
    player: "#5294C4",
    player_hovered: "#5CA6DB",
  },
  icons: {
    primary: "#467EA6",
    secondary: "#000",
    tertiary: "#fff",
    bgPrimary: "#f7f7f7",
    close: "red",
  },
  border: {
    primary: hexToRgbA("#000", 0.12),
    secondary: "#5294c3",
    active: "#5142AE",
    activeRadius: hexToRgbA("#5142AE", 0.12),
  },
  grid: {
    headerHover: "#F3F8FF",
    rowHover: "#F3F8FF",
    rowSelected: "#F3F8FF",
    fullWidthRow: "#FBFBFC",
    checkbox: "#5142AE",
    border: hexToRgbA("#000", 0.12),
  },
  leadGroups: {
    green: lightGreen,
    blue: "#5294C3",
    darkBlue: "#5142AE",
    dark: "#1E1954",
    orange: "#FF7A00",
    red: "#B94163",
    grey: "#777",
  },
  agentStatus: {
    [AgentStatusStr.InTheCall]: {
      card: lightGreen,
      text: lightGreen,
    },
    [AgentStatusStr.WaitingForTheCall]: {
      card: "#FFB800",
      text: "#FFB800",
    },
    [AgentStatusStr.FillingFeedback]: {
      card: "#C80048",
      text: "#777",
    },
    [AgentStatusStr.Offline]: {
      card: "#777",
      text: "#777",
    },
    [AgentStatusStr.Break]: {
      card: "#5294c3",
      text: "#5294c3",
    },
    [AgentStatusStr.Dialing]: {
      card: "#FFB800",
      text: "#FFB800",
    },
    [AgentStatusStr.Busy]: {
      card: "#C80048",
      text: "#C80048",
    },
  },
  modal: {
    headerBackground: "#1E1954",
    headerTextColor: "#D9D9D9",
  },
  values: {
    lightGreen,
    lightRed,
  },
};
