import React from "react";
import styled from "styled-components";
import { useAgent } from "data/user/useAgent";

const CNVLeadInfo = () => {
  const { agent } = useAgent();

  return (
    <CNVInfoWrap>
      Info:
      <iframe title={"cnv"} src={`${agent?.lead?.iframeUrl}`} width="100%" height="100%" />
    </CNVInfoWrap>
  );
};

export default CNVLeadInfo;

const CNVInfoWrap = styled.div`
  height: 80vh;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
`;
