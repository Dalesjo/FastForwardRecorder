import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileSize'
})
export class FileSizePipe implements PipeTransform {

  transform(size: number): string {
    let decimal = 0;
    let kb = 1024;
    let mb = kb*kb;
    let gb = mb*kb;

    if(size > gb)
    {
      let extension = "GB"
      return (size / gb).toFixed(decimal) + " " + extension; 
    }

    if(size > mb)
    {
      let extension = "MB"
      return (size / mb).toFixed(decimal) + " " + extension; 
    }

    let extension = "KB"
    return (size / kb).toFixed(decimal) + " " + extension; 
  }

}
