import Record from './record';

export interface TestRecord extends Record {
  '@alias': string;
  '@logicalName': string;
  '@key'?: string;
}
