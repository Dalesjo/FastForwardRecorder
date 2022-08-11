import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'daysLeft'
})
export class DaysLeftPipe implements PipeTransform {

  transform(value: number, ceil?: boolean): number {
    if (ceil) {
      return Math.ceil((value) / 86400);
    } else {
      return Math.floor((value) / 86400);
    }
  }
}
