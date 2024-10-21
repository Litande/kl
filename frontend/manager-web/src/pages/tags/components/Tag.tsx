import React, { FC } from "react";
import styled from "styled-components";
import CrossIcon from "components/icons/CrossIcon";
import { typography } from "globalStyles/theme/fonts";

type Props = {
  title: string;
  onClick?: () => void;
};

const Tag: FC<Props> = ({ title, onClick }) => {
  return (
    <Container>
      {title} <CrossIcon onClick={onClick} />
    </Container>
  );
};

export default Tag;

const Container = styled.div`
  display: flex;
  height: 36px;
  align-items: center;
  gap: 11px;
  padding: 0 8px 0;
  background: ${({ theme }) => theme.colors.bg.tag};
  border-radius: 4px;
  ${typography.body1}
`;
