import styled from "styled-components";
import { PageTitle } from "components/layout/Layout";
import CallAnalysis from "./callAnalysis/CallAnalysis";
import DonutChartVisx from "./agentsWorkMode/DonutChartVisx";
import { MercatorComponent } from "./map/charts/Mercator";
import ChartBlock from "./performanceAnalysis/ChartBlock";
import { FullScreen } from "components/fullScreen/FullScreen";
import { useEffect } from "react";
import apiService from "services/api/apiService";
import { defaultShadow } from "globalStyles/theme/border";
import { DONUT_WIDTH, FULL_WIDTH, PERFOMANCE_WIDTH } from "pages/dashboard/consts";

const Dashboard = () => {
  useEffect(() => {
    return () => {
      apiService().cancelRequests();
    };
  }, []);

  return (
    <Wrap>
      <Title>
        Dashboard <FullScreen />
      </Title>
      <Content>
        <ContentTop>
          <Container>
            <DonutChartContainer>
              <DonutChartVisx />
            </DonutChartContainer>
            <AreaChartContainer>
              <ChartBlock />
            </AreaChartContainer>
          </Container>
        </ContentTop>
        <ContentBottom>
          <MercatorComponent />
          <CallAnalysis />
        </ContentBottom>
      </Content>
    </Wrap>
  );
};

export default Dashboard;

const Title = styled(PageTitle)`
  min-width: ${FULL_WIDTH}px;
`;

const Wrap = styled.div`
  body.full-screen & {
    position: fixed;
    top: 1rem;
    left: 1rem;
    width: calc(100% - 2rem);
    height: calc(100% - 2rem);
  }
  overflow-x: scroll;
`;

const Content = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
`;

const ContentTop = styled.div`
  flex: 1;
  width: ${FULL_WIDTH}px;
`;

// Todo: align height
const ContentBottom = styled.div`
  display: flex;
  flex: 1;
  gap: 0.5rem;
  min-height: 465px;
  width: ${FULL_WIDTH}px;
`;

const Container = styled.div`
  display: flex;
  flex-direction: row;
  gap: 1rem;
`;

const DonutChartContainer = styled.div`
  width: ${DONUT_WIDTH}px;
  height: 352px;
  background: #ffffff;
  margin-left: 0.3rem;
  ${defaultShadow}
`;

const AreaChartContainer = styled.div`
  display: flex;
  height: 353px;
  width: ${PERFOMANCE_WIDTH}px;
  background: #ffffff;
  ${defaultShadow}
`;
