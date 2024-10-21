export interface IShape {
  x: number;
  y: number;
  w?: number;
  h?: number;
}

export interface IFeatureShape {
  type: "Feature";
  id: string;
  geometry: { coordinates: [number, number][][]; type: "Polygon" };
  properties: { name: string };
}
