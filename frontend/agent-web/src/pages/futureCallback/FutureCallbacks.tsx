import { useState, useEffect } from "react";
import { ContentWrap, PageWrap } from "components/layout/AgentLayout";
import FutureItem from "components/futureCallback/FutureItem";
import PageTitle from "components/layout/PageTitle";
import agentApi from "services/api/agentApi";

const FutureCallbacks = () => {
  const [data, setData] = useState([]);

  useEffect(() => {
    agentApi.getCallbacks().then(({ data }) => setData(data.items));
  }, []);

  return (
    <PageWrap>
      <PageTitle label={"Future Callbacks"} />
      <ContentWrap>
        {data.map(data => {
          return <FutureItem key={data.leadId} {...data} />;
        })}
      </ContentWrap>
    </PageWrap>
  );
};

export default FutureCallbacks;
