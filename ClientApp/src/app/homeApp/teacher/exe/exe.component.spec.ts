import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExeComponent } from './exe.component';

describe('ExeComponent', () => {
  let component: ExeComponent;
  let fixture: ComponentFixture<ExeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
