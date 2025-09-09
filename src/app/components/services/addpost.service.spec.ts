/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { AddpostService } from './addpost.service';

describe('Service: Addpost', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AddpostService]
    });
  });

  it('should ...', inject([AddpostService], (service: AddpostService) => {
    expect(service).toBeTruthy();
  }));
});
