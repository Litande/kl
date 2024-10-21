import { IFutureCallback } from "components/futureCallback/types";
import styled from "styled-components";
import moment from "moment";
import { typography } from "globalStyles/theme/fonts";
import { colors } from "globalStyles/theme/palette";

type LabelItemProps = {
  header: string;
  label: string | number;
};

const LabelItem = ({ header, label }: LabelItemProps) => {
  return (
    <ItemWrap>
      <ItemHeaderWrap>{header}</ItemHeaderWrap>
      <ItemValueWrap>{label}</ItemValueWrap>
    </ItemWrap>
  );
};

const FutureItem = (data: IFutureCallback) => {
  return (
    <Wrap>
      <HeaderWrap>
        <LabelHeaderWrap>
          <ItemHeaderWrap>Next Call:</ItemHeaderWrap>
          <NextCallWrap>
            <h1>{data.nextCallAt && moment(data.nextCallAt).format("HH:mm")}</h1>
          </NextCallWrap>
        </LabelHeaderWrap>
        <LabelHeaderWrap>
          <ItemHeaderWrap>Last Call:</ItemHeaderWrap>
          <LastCallWrap>
            <h3>{data.lastCallAt && moment(data.lastCallAt).format("DD.MM.YY")}</h3>
          </LastCallWrap>
        </LabelHeaderWrap>
      </HeaderWrap>
      <ContentWrap>
        <LabelItem label={`ID ${data.leadId}`} header={`CID`} />
        <LabelItem label={data.phone} header={`PHONE`} />
        <LabelItem label={data.countryCode} header={`COUNTRY`} />
        <LabelItem label={data.email} header={`EMAIL`} />
        <LabelItem
          label={data.registeredAt && moment(data.registeredAt).format("DD.MM.YY")}
          header={`REG. DATE`}
        />
        <LabelItem
          label={
            data.totalCallsSecond && moment.utc(data.totalCallsSecond * 1000).format("HH:mm:ss")
          }
          header={`TOTAL CALLS`}
        />
        <LabelItem label={data.firstName} header={`FIRST NAME`} />
        <LabelItem label={data.lastName} header={`LAST NAME`} />
        <LabelItem label={data.leadStatus} header={`SALES STATUS`} />
        <LabelItem label={data.weight} header={`WEIGHT`} />
        <LabelItem label={data.campaign} header={`CAMPAIGN`} />
      </ContentWrap>
    </Wrap>
  );
};

export default FutureItem;

const Wrap = styled.div`
  cursor: pointer;
  display: flex;
  flex-direction: column;
  &:hover {
    background: ${colors.bg.active};
  }
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
  padding: 1rem;
`;

const HeaderWrap = styled.div`
  display: flex;
  flex-direction: row;
  padding-bottom: 1rem;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
  width: 100%;
  gap: 2rem;
`;

const ContentWrap = styled.div`
  display: flex;
  flex-wrap: wrap;
  margin-top: 1rem;
`;

const ItemWrap = styled.div`
  display: flex;
  flex-direction: column;
  margin-bottom: 1rem;
  flex: 0 0 16.6%;
  white-space: nowrap;
`;

const ItemHeaderWrap = styled.label`
  padding-bottom: 3px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  text-transform: uppercase;
  display: flex;
  align-items: flex-end;
`;

const ItemValueWrap = styled.div`
  ${typography.body1}
`;

const LabelHeaderWrap = styled.div`
  display: flex;
  flex-direction: row;
  gap: 0.5rem;
`;

const NextCallWrap = styled.div`
  color: ${colors.bg.success};
  align-items: flex-end;
`;

const LastCallWrap = styled.div`
  display: flex;
  align-items: flex-end;
`;
