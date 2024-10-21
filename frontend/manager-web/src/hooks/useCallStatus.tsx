import { useEffect, useState } from "react";
import CallService from "services/callService/CallService";

const useCallStatus = () => {
  const [callStatus, setCallStatus] = useState(CallService.getInstance().status?.getValue());

  useEffect(() => {
    const subscribe = CallService.getInstance().status.subscribe(value => {
      setCallStatus(value);
    });
    return () => {
      subscribe.unsubscribe();
    };
  }, []);

  return { callStatus };
};

export default useCallStatus;
