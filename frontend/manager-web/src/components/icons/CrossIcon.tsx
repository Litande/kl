import React, { FC } from "react";
import styled from "styled-components";

type Props = {
  onClick: (e: React.MouseEvent<HTMLElement>) => void;
};

const CrossIcon: FC<Props> = ({ onClick }) => (
  <Container onClick={onClick} className="icon-close" />
);

export default CrossIcon;

const Container = styled.i`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 16px;
  height: 16px;
  line-height: 16px;
  text-align: center;
  font-size: 12px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border-radius: 50%;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  cursor: pointer;

  &:before {
    position: relative;
    top: 0.5px;
    right: -0.5px;
  }
`;
