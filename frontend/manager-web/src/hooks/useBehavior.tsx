/* eslint-disable @typescript-eslint/no-explicit-any */
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";

const memoizedResult = (_, fn) => fn();

function useBehavior<
  Params extends {
    [K in keyof Params]: ReturnType<Params[K] | ReturnType<any>>;
  }
>(behaviors: Params): { [K in keyof Params]: ReturnType<Params[K]> } {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  type Key = keyof Params;
  return memoizedResult(behaviors, () => {
    const keys = Object.keys(behaviors) as Array<Key>;
    return keys.reduce(
      (acc, key) => ({
        ...acc,
        [key]: behaviors[key]({ dispatch, navigate }),
      }),
      {}
    );
  });
}

export default useBehavior;
