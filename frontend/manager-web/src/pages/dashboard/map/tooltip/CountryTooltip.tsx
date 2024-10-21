import styled from "styled-components";
import { IFeatureShape, IShape } from "pages/dashboard/map/types";
import { ICountryItem } from "pages/dashboard/callAnalysis/types";
import CountryFlag from "components/flag/CountryFlag";

export interface ICountryTooltip extends IShape {
  data?: ICountryItem;
  feature: Pick<IFeatureShape, "id">;
}

const CountryTooltip = ({ data, x, y, feature }: ICountryTooltip) => {
  return (
    <Wrap x={x} y={y}>
      <Container>
        <FlagWrap>
          <CountryFlag iso3={data?.code || feature.id} />
        </FlagWrap>
        <StatisticWrap>
          <StatisticItemWrap>
            <div>AR:</div>
            <div>{data?.rate}%</div>
          </StatisticItemWrap>
          <StatisticItemWrap>
            <div>AT:</div>
            <div>{data?.avgTime}</div>
          </StatisticItemWrap>
        </StatisticWrap>
      </Container>
      <PointerWrap>
        <LineShape />
        <RoundShape>{data?.amount}</RoundShape>
      </PointerWrap>
    </Wrap>
  );
};

export default CountryTooltip;

interface IXY {
  x: number;
  y: number;
}

const Wrap = styled.div<IXY>`
  position: absolute;
  width: 129px;
  height: 43px;
  left: ${props => {
    return props.x - 65 + "px";
  }};
  top: ${props => {
    return props.y - 69 + "px";
  }};
  background: #ffffff;
  box-shadow: 0 4px 4px rgba(0, 0, 0, 0.25);
  border-radius: 8px;
  -webkit-user-select: none;
  -ms-user-select: none;
  user-select: none;
`;

const Container = styled.div`
  width: 129px;
  height: 43px;
  display: flex;
  flex-direction: row;
  margin: 0.5rem;
`;

const FlagWrap = styled.div`
  width: 36px;
  height: 21px;
`;

const StatisticWrap = styled.div`
  display: flex;
  flex-direction: column;
  margin-left: 8px;
  width: 55%;
  gap: 4px;
  align-content: space-between;
  font-size: 10px;
  line-height: 12px;
  font-weight: 300;
  font-family: "Inter regular", sans-serif;
`;

const StatisticItemWrap = styled.div`
  display: flex;
  justify-content: space-between;
`;

const PointerWrap = styled.div`
  position: absolute;
  flex-direction: column;
  display: flex;
  bottom: -48px;
  left: 60px;
`;

const LineShape = styled.div`
  height: 18px;
  width: 2px;
  background: white;
`;

const RoundShape = styled.div`
  width: 31px;
  height: 31px;
  border-radius: 50%;
  background: #bcb6db;
  border: 4px white solid;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-left: -19px;
  font-family: "Inter medium", sans-serif;
  font-style: normal;
  font-weight: 500;
  font-size: 11px;
  line-height: 13px;
`;
