import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";
import { ContentWrap, PageWrapContent } from "components/layout/AgentLayout";

import WaitingSVG from "icons/components/WaitingSVG";
import PageTitle from "components/layout/PageTitle";

const WaitingsState = () => {
  return (
    <PageWrapContent>
      <PageTitle label={"Dialing"} />
      <ContentWrap>
        <WrapContent>
          <WaitingSVG />
        </WrapContent>
      </ContentWrap>
    </PageWrapContent>
  );
};

export default WaitingsState;

const WrapContent = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  text-align: left;
  ${typography.subtitle2}
`;
