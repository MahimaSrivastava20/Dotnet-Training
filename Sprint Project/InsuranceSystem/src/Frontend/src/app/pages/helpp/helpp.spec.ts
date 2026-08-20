import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Helpp } from './helpp';

describe('Helpp', () => {
  let component: Helpp;
  let fixture: ComponentFixture<Helpp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Helpp],
    }).compileComponents();

    fixture = TestBed.createComponent(Helpp);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
