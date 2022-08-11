import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'hoursLeft'
})
export class HoursLeftPipe implements PipeTransform {

  transform(value: number, args?: any): number {
    return Math.floor((value % 86400) / 3600);
  }

}
