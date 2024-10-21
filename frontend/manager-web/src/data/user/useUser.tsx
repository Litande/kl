import { useContext } from "react";
import { AuthContext } from "data/user/AuthContext";

const useUser = () => {
  const { user } = useContext(AuthContext);
  return user;
};

export default useUser;
