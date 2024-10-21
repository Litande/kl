import React from "react";
import styled from "styled-components";

type Attribute = {
  key: string;
  label: string;
  value: number | string;
};

export interface ICardData {
  name: string;
  attributes: Array<Attribute>;
}

export interface IInfoCard {
  className?: string;
  data: ICardData;
  color?: string;
}

const InfoCard: React.FC<IInfoCard> = ({ data, className, color }) => {
  return (
    <Container className={className}>
      <Name paintTO={color}>{data.name}</Name>
      <InfoBlocksContainer>
        {data.attributes.map((attr: Attribute) => (
          <InfoBlock key={attr.key}>
            <InfoBlockTop>{attr.label}</InfoBlockTop>
            <InfoBlockBottom>{attr.value}</InfoBlockBottom>
          </InfoBlock>
        ))}
      </InfoBlocksContainer>
    </Container>
  );
};

export default InfoCard;

const Container = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 8px;
`;

const Name = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 14px 12px;
  ${({ theme }) => theme.typography.subtitle2}
  color: ${({ theme, paintTO: color }) => color || theme.colors.btn.secondary_pressed};
`;

const InfoBlocksContainer = styled.div`
  display: flex;
  padding: 14px 14px 16px;
  background: ${({ theme }) => "#F7F7F7"};
  border-radius: 0 0 8px 8px;
`;
const InfoBlock = styled.div`
  flex: 1;
  display: flex;
  flex-direction: column;
`;
const InfoBlockTop = styled.div`
  margin: 0 0 3px;
  ${({ theme }) => theme.typography.smallText3};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
`;
const InfoBlockBottom = styled.div`
  ${({ theme }) => theme.typography.smallText3};
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.fg.secondary};
`;
