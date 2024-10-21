import { useCallback, useState } from "react";

// Parameter is the boolean, with default "false" value
const useToggle = (initialState = false): [boolean, () => void] => {
  // Initialize the state
  const [isActive, setIsActive] = useState<boolean>(initialState);
  // Define and memorize toggler function in case we pass down the comopnent,
  // This function change the boolean value to it's opposite value
  const toggle = useCallback((): void => setIsActive(state => !state), []);
  return [isActive, toggle];
};
export default useToggle;
