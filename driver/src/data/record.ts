export default interface Record {
  [attribute: string]: number | string | unknown | unknown[];
}

export const recordInternalProperties = [
  '@alias',
  '@logicalName',
  '@key',
  '@bpf',
  '@extends',
  '@activestageid',
];

export function exludeInternalPropertiesFromPayload(record: Record) {
  const updatedRecord = { ...record } as Record;
  Object.keys(record).forEach((key) => {
    if (recordInternalProperties.includes(key)) {
      delete updatedRecord[key];
    }
  });
  return updatedRecord;
}
