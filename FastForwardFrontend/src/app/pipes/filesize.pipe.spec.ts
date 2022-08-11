import { FileSizePipe } from './filesize.pipe';

describe('fileSizePipe', () => {
  it('create an instance', () => {
    const pipe = new FileSizePipe();
    expect(pipe).toBeTruthy();
  });
});
