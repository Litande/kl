import styled from "styled-components";
import { LegendItem, LegendLabel } from "@visx/legend";
import { LegendOrdinal } from "@visx/legend";
import { scaleOrdinal } from "@visx/scale";
import { LegendProps } from "./types";

export const LegendOrdinalComponent = ({ data, setActive }: LegendProps) => {
  const ordinalColorScale = scaleOrdinal({
    domain: data,
    range: data,
  });

  return (
    <LegendOrdinal scale={ordinalColorScale} labelFormat={prop => `${prop.symbol.toUpperCase()}`}>
      {labels => (
        <LegendItemsContainer>
          {labels.map((labelData, i) => {
            return (
              <StyledLegendItem
                key={`legend-quantile-${i}`}
                onClick={() => {
                  setActive(labelData.value);
                }}
              >
                <svg width={28} height={28}>
                  <circle fill={labelData.value.color} r={14 / 2} cx={28 / 2} cy={28 / 2} />
                </svg>
                <StyledLegendLabel fontSize={14} color="grey" lineheight={17} flex="3 1 170px">
                  {labelData.text}
                </StyledLegendLabel>
                <StyledLegendLabel fontSize={15} lineheight={18} align="right">
                  {labelData.datum.amount}
                </StyledLegendLabel>
              </StyledLegendItem>
            );
          })}
        </LegendItemsContainer>
      )}
    </LegendOrdinal>
  );
};

const LegendItemsContainer = styled.div`
  display: "flex";
  flex-direction: "column";
  gap: 20px;
  margin-left: 7px;
  margin-right: 10px;
`;

const StyledLegendItem = styled(LegendItem)`
  ${({ theme }) => theme.typography.body1};
  font-weight: 400;
  font-size: 14px;
  line-height: 17px;
  text-transform: uppercase;
  gap: 10px;
  display: flex;
  align-items: center;
  justify-content: space-around;
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
  height: 50px;
`;

const StyledLegendLabel = styled(LegendLabel)<{
  fontSize: number;
  lineheight: number;
  color?: string;
}>`
  ${({ theme }) => theme.typography.body1};
  font-weight: 500;
  font-size: ${({ fontSize }) => fontSize + "px"};
  line-height: ${({ lineheight }) => lineheight + "px"};
  ${({ color }) => color && `color: ${color};`}
  text-transform: uppercase;
  width: max-content;
`;
