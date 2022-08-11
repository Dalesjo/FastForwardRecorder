import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateToSeconds'
})
export class DateToSecondsPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    var timestamp = Date.parse(value);
    var now = Date.now();
    var left = timestamp - now;

    if (left > 1000) {
      return Math.floor(left / 1000);
    } else {
      return 0;
    }
  }

}
