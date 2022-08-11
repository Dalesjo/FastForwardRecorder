import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'zeroPad'
})
export class ZeroPadPipe implements PipeTransform {

  transform(item: number): string {
    return (String('0').repeat(2) + item).substr((2 * -1), 2);
  }

}
