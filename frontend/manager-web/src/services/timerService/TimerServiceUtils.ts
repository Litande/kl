/*
 *   We can try to use it not uuid package - maybe it will work
 * */
export const getUniqueId = () =>
  String(Date.now().toString(32) + Math.random().toString(16)).replace(/\./g, "");
