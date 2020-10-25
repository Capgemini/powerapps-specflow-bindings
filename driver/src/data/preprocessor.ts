import { Record } from './record';

export default abstract class Preprocessor {
  abstract preprocess(data: Record): Record;
}
