import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'minutesLeft'
})
export class MinutesLeftPipe implements PipeTransform {

  transform(value: number, args?: any): number {
    return Math.floor((value % 3600) / 60);
  }

}
