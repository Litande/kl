export function getLocalStorageItem(key, parse = false) {
  const value = localStorage.getItem(key);
  if (parse) {
    return JSON.parse(value);
  }
  return value;
}

export function setLocalStorageItem(key, value) {
  return localStorage.setItem(key, value);
}

export function removeLocalStorageItem(key) {
  return localStorage.removeItem(key);
}
