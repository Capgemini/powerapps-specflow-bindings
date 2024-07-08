export default interface Record {
  [attribute: string]: number | string | unknown | unknown[];
}

export const recordInternalProperties = [
  '@alias',
  '@logicalName',
  '@key',
];
