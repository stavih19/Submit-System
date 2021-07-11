import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoCheckComponent } from './auto-check.component';

describe('AutoCheckComponent', () => {
  let component: AutoCheckComponent;
  let fixture: ComponentFixture<AutoCheckComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutoCheckComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
