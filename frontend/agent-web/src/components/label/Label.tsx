import React from "react";
import styled from "styled-components";

interface ILabel {
  header?: string;
  label: string;
  styles?: React.CSSProperties;
}

const Label = ({ header, label, styles }: ILabel) => {
  return (
    <Container style={styles}>
      {label && <HeaderWrap>{header}</HeaderWrap>}
      <h1>{label}</h1>
    </Container>
  );
};

export default Label;

const Container = styled.div`
  display: flex;
  flex-direction: column;
`;

const HeaderWrap = styled.label`
  padding-bottom: 3px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  text-transform: uppercase;
`;
