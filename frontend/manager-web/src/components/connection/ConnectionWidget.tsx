import { FC } from "react";
import styled from "styled-components";

import IconConnections from "components/connection/icons/IconConnections";
import { FillMapByStatus } from "components/connection/consts";
import { typography } from "globalStyles/theme/fonts";
import useStatusConnection from "components/connection/useStatusConnection";

type Props = {
  isFullInfo?: boolean;
};

const ConnectionWidget: FC<Props> = ({ isFullInfo = true }) => {
  const { status } = useStatusConnection();

  return (
    <Wrap>
      <IconWrap>
        <IconConnections status={status} />
      </IconWrap>
      <TextWrap isFullInfo={isFullInfo}>{FillMapByStatus[status].description}</TextWrap>
    </Wrap>
  );
};

export default ConnectionWidget;

const Wrap = styled.div`
  display: flex;
  flex-direction: row;
  gap: 1rem;
`;

const IconWrap = styled.span``;

const TextWrap = styled.span<{ isFullInfo?: boolean }>`
  ${typography.smallText1}
  color: rgba(0, 0, 0, 0.6);
  white-space: nowrap;
  overflow: hidden;
  transition: width 0.5s, margin 0.6s;

  width: ${({ isFullInfo }) => (isFullInfo ? "150px" : "0")};
`;
