export const checkEnumType = (value, enumValue) => {
  return Object.values(enumValue).includes(value);
};

export const getValueFromEnum = <T>(value, enumValue, defaultValue: T = null): T => {
  const isEnum = checkEnumType(value, enumValue);
  return isEnum ? value : defaultValue;
};
