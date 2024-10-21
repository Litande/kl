// eslint-disable-next-line check-file/filename-naming-convention
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { GeoMap } from "./AirbnbMap";

export default {
  title: "Example/AirBnBMap",
  component: GeoMap,
  parameters: {
    layout: "fullscreen",
  },
} as ComponentMeta<typeof GeoMap>;

const Template: ComponentStory<typeof GeoMap> = () => <GeoMap />;

export const WorldMap = Template.bind({});
WorldMap.args = {};
