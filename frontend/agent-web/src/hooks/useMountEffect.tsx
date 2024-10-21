import { useEffect, useState } from "react";

const useMountEffect = () => {
  const [updater, setUpdater] = useState(0);
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    if (updater > 0) {
      setIsMounted(true);
    }
  }, [updater]);

  useEffect(() => {
    setUpdater(updater + 1);
  }, []);

  return {
    isMounted,
  };
};

export default useMountEffect;
