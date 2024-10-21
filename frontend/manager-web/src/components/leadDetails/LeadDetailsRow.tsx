import React from "react";
import styled from "styled-components";

import { ILead } from "types";

type ILeadAdditionalDetails = {
  data: ILead;
  dividerWidth?: number;
};

function LeadAdditionalDetails({ data, dividerWidth }: ILeadAdditionalDetails) {
  const keys = [
    "leadName",
    "brandName",
    "affiliateId",
    "registrationTime",
    "callDuration",
    "phoneNumber",
    "country",
  ];

  const keyToLabel = {
    affiliateId: "Aff.ID",
    registrationTime: "Reg. Time",
  };

  return (
    <Container>
      {keys.map(
        key =>
          data[key] && (
            <DetailItem key={key} dividerWidth={dividerWidth}>
              {keyToLabel[key] ? `${keyToLabel[key]} ` : ""}
              {data[key]}
            </DetailItem>
          )
      )}
    </Container>
  );
}

export default LeadAdditionalDetails;

const Container = styled.div`
  padding: 0 0 0 30px;
  ${({ theme }) => theme.typography.smallText1};
  word-break: break-word;
`;

const DetailItem = styled.span<{ dividerWidth?: number }>`
  margin-right: ${({ dividerWidth }) => (dividerWidth ? `${dividerWidth}px` : "22px")};
  word-break: break-all;
  font-weight: 400;

  &:after {
    display: inline-block;
    content: "|";
    margin: 0 0 0 ${({ dividerWidth }) => (dividerWidth ? `${dividerWidth}px` : "22px")};
  }

  &:last-child:after {
    content: "";
  }
`;
